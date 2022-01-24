namespace Ragusa.Xml

[<RequireQualifiedAccess>]
module Encoder =
    open Types
    open System.Xml.Linq

    let inline primitive<'T> (value: 'T): XmlValue =
        XElement(typeof<'T>.Name, value)

    let inline object (rootName: string) (values : (string * XmlValue) seq): XmlValue =
        values
        |> Seq.map
            (fun (key, value) -> XElement(key, value))
        |> fun x -> XElement(rootName, x)