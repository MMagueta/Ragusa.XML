namespace Ragusa.Xml

[<RequireQualifiedAccess>]
module Encoder =
    open Types
    open System.Xml.Linq

    let inline primitive<'T> (name: string) (value: 'T): XElement =
        XElement (name, value)

    let inline complex (rootName: string) (values : (string * XElement) seq): XElement =
        values
        |> Seq.map
            (fun (key, value) -> XElement(key, value))
        |> fun x -> XElement (rootName, x)
