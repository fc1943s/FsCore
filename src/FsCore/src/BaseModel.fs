namespace FsCore


module BaseModel =
    type Username = Username of alias: string
    type Color = Color of hex: string
    type StoreRoot = StoreRoot of name: string
    type Collection = Collection of collection: string
    type FileId = FileId of guid: TicksGuid
    type DeviceId = DeviceId of guid: TicksGuid
    type Ping = Ping of ticksText: string

    type Username with
        static member inline Value (Username username) = username

    type StoreRoot with
        static member inline Value (StoreRoot name) = name

    type Collection with
        static member inline Value (Collection collection) = collection

    type Color with
        static member inline Value (Color hex) = hex
        static member inline Default = Color "#000000"

    type FileId with
        static member inline NewId () = FileId (Guid.newTicksGuid ())
        static member inline Value (FileId guid) = guid

    type DeviceId with
        static member inline NewId () = DeviceId (Guid.newTicksGuid ())
        static member inline Value (DeviceId guid) = guid

    type Ping with
        static member inline Value (Ping ticks) = int64 ticks
