module Expression

open System.Xml.Linq

[<RequireQualifiedAccess>]
type XMLType =
| Complex of XElement
| Primitive of string*obj

// type XMLBuilder() =
//     member _.Yield(x) = [x]
//     member _.Combine (a,b) =
//         List.concat [a; b]
//     member _.Delay(f) = f()
//     member _.Run(l:(string*obj) list) =
//         XMLOption.Element (XElement("Root", l |> Seq.map (fun (name, value) -> XElement(name, value))))
//     member _.Zero() = []
//     member _.Bind(m: XMLOption, f) =
//         f m

type Items = Items of XMLType list

type XMLBuilder(rootName: string) =
    member x.Yield(vars) = Items [],vars

    [<CustomOperation("encode",MaintainsVariableSpace=true)>]
    member x.Encode((Items current,vars), [<ProjectionParameter>]f) =
        Items (current @ [f vars]),vars

    member x.Run(l: Items,_) = 
        XElement(rootName, match l with | Items x -> x |> Seq.map (fun x -> match x with | XMLType.Complex x -> x | XMLType.Primitive (name,value) -> XElement(name, value)))


let xml rootName = new XMLBuilder(rootName)

let test = 
    xml "Root" {
        encode (XMLType.Primitive ("A", 1))
        encode (XMLType.Primitive ("B", 2))
        encode 
            (XMLType.Complex 
                (xml "Element2" {
                    encode (XMLType.Primitive ("C", 1))
                    encode (XMLType.Primitive ("D", 2))
                }))
    }

type ABC = 
    { A: string
      B: int }
    with
        static member Encoder (elem: ABC): XElement =
            xml "ABC" {
                encode (XMLType.Primitive ("A", elem.A))
                encode (XMLType.Primitive ("B", elem.B))
            }