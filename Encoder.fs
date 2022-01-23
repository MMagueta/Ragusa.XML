namespace Ragusa.Xml

[<RequireQualifiedAccess>]
module Encode =
    open Types
    open System.Xml.Linq


    let inline string (value : string) : XmlValue =
        XElement("String", value)

    let inline float (value : float) : XmlValue =
        XElement("Float", value)

    let inline bool (value : bool) : XmlValue =
        XElement("Bool", value)

    let inline object (rootName: string) (values : (string * XmlValue) seq): XmlValue =
        values
        |> Seq.map
            (fun (key, value) -> XElement(key, value))
        |> fun x -> XElement(rootName, x)