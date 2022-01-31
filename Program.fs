namespace Ragusa.Xml

module Router =
    open Saturn
    open Giraffe
    open Microsoft.AspNetCore.Http
    open System.Xml.Linq

    [<CLIMutable>]
    type Health = {
        Text : string
        Time : System.DateTime
        Version : float
        Flags : string array
    }

    let health =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                // return! ctx.WriteXmlAsync { Text = "Development Build"; Time = System.DateTime.Now; Version = 1.0; Flags = [| "xml"; "ident" |] }
                ctx.SetContentType "text/xml; charset=utf-8"
                // return! (Ragusa.Xml.Giraffe.Serializer.Respond (Encode.object "object1" ["object", Encode.object "object2" ["abc", Encode.string "def"; "hij", Encode.bool true]; "xyz", Encode.float 123.456])) next ctx
                // printfn "%A" (Expression.test)
                let test: Expression.ABC = {A = "Field A"; B = 1}
                return! (Ragusa.Xml.Giraffe.Serializer.Respond (Expression.ABC.Encoder test)) next ctx
                // return! (Ragusa.Xml.Giraffe.Serializer.Respond (Encoder.primitive<string> "test string")) next ctx
                // return! (Ragusa.Xml.Giraffe.Serializer.Respond (Encoder.primitive { Text = "Development Build"; Time = System.DateTime.Now; Version = 1.0; Flags = [| "xml"; "ident" |] })) next ctx
            }

    [<CLIMutable>]
    type Developer =
        {
            Name   : string
            Editor : string
        }

    let submitDeveloper =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let! dev = ctx.BindXmlAsync<Developer>()
                return! xml dev next ctx
            }

    let mainRouting =
        choose [ GET
                 >=> choose [ route "health" >=> health ]
                 POST
                 >=> choose [ route "submitDeveloper" >=> submitDeveloper ]
                 setStatusCode 404
                 >=> RequestErrors.BAD_REQUEST "Not found!" ]

    let entryRouting = router { forward "/" mainRouting }

module API = 
    open Saturn
    open System.Xml
    open System.Text

    let app =
        application {
            url "http://0.0.0.0:8085"
            use_router Router.entryRouting
            memory_cache
            use_static "public"
            use_gzip
            use_xml_serializer (Giraffe.SystemXml.Serializer (XmlWriterSettings(Encoding = Encoding.UTF8, Indent = true)))
        }

    [<EntryPointAttribute>]
    let main _ =
        run app
        0