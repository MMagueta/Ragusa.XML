namespace Ragusa.Xml

module Types = 
    open System.Xml.Linq

    [<RequireQualifiedAccess>]
    type XMLEncoder<'T, 'E> =
    | Success of 'T
    | Failure of 'E

    let inline (>>=) (value: XMLEncoder<'T, 'E>) (f: 'T -> XMLEncoder<'U, 'E>) =
        match value with
        | XMLEncoder.Success element -> f element
        | XMLEncoder.Failure error -> XMLEncoder.Failure error

    type Items = Items of (XName*obj list) list

    type XMLEncoderBuilder() =
        member _.Yield(vars) = Items [],vars

        [<CustomOperation("encode", MaintainsVariableSpace=true)>]
        member _.Push((Items current,vars), [<ProjectionParameter>]f) =
            Items (current @ [f vars]),vars

        member _.Run(Items l, _) = XElement("Root", l |> Seq.map (fun (name, elem) -> XElement(name, elem |> Array.ofList)))

    let xmlEncoder = XMLEncoderBuilder()