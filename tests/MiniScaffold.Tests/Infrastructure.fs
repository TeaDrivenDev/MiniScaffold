namespace Infrastructure


module Dotnet =
    open Fake.Core
    open Fake.DotNet
    let failOnBadExitAndPrint (p : ProcessResult) =
        if p.ExitCode <> 0 then
            p.Errors |> Seq.iter Trace.traceError
            failwithf "failed with exitcode %d" p.ExitCode


    module New =
        let cmd opt args =
            let args = args |> String.concat " "
            DotNet.exec opt "new" args
            |> failOnBadExitAndPrint
        let install name =
            let args = [
                sprintf "-i %s" name
            ]
            cmd id args
        let uninstall name =
            let args = [
                sprintf "-u %s" name
            ]
            cmd id args


module Disposables =
    open System

    let dispose (disposable : #IDisposable) = disposable.Dispose()

    [<AllowNullLiteral>]
    type DisposableDirectory (directory : string) =

        static member Create() =
            let tempPath = IO.Path.Combine(IO.Path.GetTempPath(), Guid.NewGuid().ToString("n"))
            IO.Directory.CreateDirectory tempPath |> ignore

            new DisposableDirectory(tempPath)
        member x.Directory = directory
        member x.DirectoryInfo = IO.DirectoryInfo(directory)

        interface IDisposable with
            member x.Dispose() =
                IO.Directory.Delete(x.Directory, true)
