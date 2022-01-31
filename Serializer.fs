namespace Ragusa.Xml

module Giraffe =
    open Microsoft.AspNetCore.Http
    open Giraffe
    open System.Xml.Linq
    
    [<RequireQualifiedAccess>]
    type Serializer () =
        class
            static member Respond (body: XElement) =
                fun (next: HttpFunc) (ctx: HttpContext) ->
                    task {
                        ctx.SetContentType "text/xml; charset=utf-8"
                        return! ctx.WriteXmlAsync(body)
                    }
        end