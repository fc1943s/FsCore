namespace FsCore

open Expecto
open Expecto.Flip


module Tests =
    [<Tests>]
    let tests =
        testList
            "Core"
            [
                testList
                    "Guid"
                    [
                        test "TicksGuid" {
                            let ticksGuid = Guid.newTicksGuid ()
                            let ticks = ticksGuid |> Guid.ticksFromGuid
                            let newTicksGuid = ticks |> Guid.newGuidFromTicks
                            let newTicks = newTicksGuid |> Guid.ticksFromGuid
                            ticks |> Expect.equal "" newTicks
                        }
                    ]
            ]
