namespace FsCore

open System
open System.Collections.Generic
open Microsoft.FSharp.Reflection


[<AutoOpen>]
module CoreMagic =
    type TicksGuid = Guid

    let inline getLocals () = ""


module DateTime =
    let inline ticksDiff ticks =
        (TimeSpan (DateTime.Now.Ticks - ticks))
            .TotalMilliseconds


module Enum =
    let inline ToList<'T> () =
        (Enum.GetValues typeof<'T> :?> 'T [])
        |> Array.toList

    let inline name<'T> (value: 'T) = Enum.GetName (typeof<'T>, value)


module Function =
    let inline memoizeLazy fn =
        let result = lazy (fn ())
        fun () -> result.Value


module Guid =
    let inline ticksFromGuid (ticksGuid: TicksGuid) =
        let ticks = string ticksGuid
        int64 $"{ticks.[0..7]}{ticks.[9..12]}{ticks.[14..17]}{ticks.[19..20]}"

    let inline newGuidFromTicks (ticks: int64) =
        let guid = Guid.NewGuid () |> string
        let ticks = string ticks
        TicksGuid $"{ticks.[0..7]}-{ticks.[8..11]}-{ticks.[12..15]}-{ticks.[16..17]}{guid.[21..]}"

    let inline newTicksGuid () = newGuidFromTicks DateTime.Now.Ticks

    let inline (|Valid|Invalid|) (str: string) =
        match Guid.TryParse str with
        | true, guid -> Valid guid
        | _ -> Invalid


module ListIter =
    let inline length (target: ^X when ^X: (member length : int)) = (^X: (member length : int) target)

    let inline item (target: ^X when ^X: (member item : int -> ^Y)) (index: int) =
        (^X: (member item : int -> ^Y) target, index)


module Map =
    let inline singleton key value =
        key, value |> List.singleton |> Map.ofSeq

    let inline keys (source: Map<'Key, 'T>) : seq<'Key> = source |> Map.toSeq |> Seq.map fst

    let inline values (source: Map<'Key, 'T>) : seq<'T> = source |> Map.toSeq |> Seq.map snd

    let inline unionWith combiner (source1: Map<'Key, 'Value>) (source2: Map<'Key, 'Value>) =
        Map.fold
            (fun m k v' ->
                Map.add
                    k
                    (match m |> Map.tryFind k with
                     | Some v -> combiner v v'
                     | None -> v')
                    m)
            source1
            source2

    let inline union (source: Map<'Key, 'T>) (altSource: Map<'Key, 'T>) =
        unionWith (fun x _ -> x) source altSource

    let inline mapKeys f (x: Map<'Key, 'T>) =
        x |> Map.toSeq |> Seq.map f |> Map.ofSeq

    let inline choose fn map =
        map
        |> Seq.choose (fun (KeyValue (k, v)) -> fn (k, v))
        |> Map.ofSeq

    let inline mapValues f (x: Map<'Key, 'T>) = Map.map (fun _ -> f) x

    let inline tryFindDictionary key (map: Dictionary<_, _>) =
        match map.TryGetValue key with
        | true, value -> Some value
        | _ -> None


module Object =
    let inline compare<'T> (a: 'T) (b: 'T) = (unbox a) = (unbox b)

    let inline newDisposable fn =
        { new IDisposable with
            member _.Dispose () = fn ()
        }


module Option =
    let inline ofObjUnboxFn<'T> (getValue: unit -> 'T) =
        getValue ()
        |> unbox
        |> Option.ofObj
        |> Option.map (fun _ -> getValue ())

    let inline ofObjUnbox<'T> (value: 'T) =
        // ofObjUnboxFn (fun () -> value)
        value
        |> unbox
        |> Option.ofObj
        |> Option.map (fun _ -> value)


module Reflection =
    let inline unionCases<'T> =
        FSharpType.GetUnionCases typeof<'T>
        |> Array.toList
        |> List.map (fun unionCaseInfo -> FSharpValue.MakeUnion (unionCaseInfo, [||]) :?> 'T)


module Seq =
    let inline intersperse sep list =
        seq {
            let mutable notFirst = false

            for element in list do
                if notFirst then yield sep

                yield element
                notFirst <- true
        }

    let inline ofItems items =
        seq {
            for i = 0 to (ListIter.length items) - 1 do
                yield (ListIter.item items) i
        }

    let inline random items =
        let len = items |> Seq.length
        items |> Seq.item (Random().Next (0, len))


module List =
    let inline removeAt index list =
        list
        |> List.indexed
        |> List.filter (fun (i, _) -> i <> index)
        |> List.map snd

    let inline intersperse element source : list<'T> =
        source
        |> List.toSeq
        |> Seq.intersperse element
        |> Seq.toList


module String =
    let inline split (separator: string) (str: string) = str.Split separator
    let inline substring startIndex length (str: string) = str.Substring (startIndex, length)
    let inline replace (a: string) (b: string) (str: string) = str.Replace (a, b)
    let inline substringFrom startIndex (str: string) = str.Substring startIndex

    let inline (|Valid|Whitespace|Null|) (str: string) =
        match str with
        | null -> Null
        | str when String.IsNullOrWhiteSpace str -> Whitespace
        | str -> Valid str

    let inline (|Invalid|_|) (str: string) =
        match str with
        | Whitespace
        | Null -> Some Invalid
        | _ -> None
