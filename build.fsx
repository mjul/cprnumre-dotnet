#r "paket:
nuget Fake.DotNet.Cli
nuget Fake.IO.FileSystem
nuget Fake.Core.Target //"
#load ".fake/build.fsx/intellisense.fsx"
open Fake.Core
open Fake.DotNet
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open Fake.Core.TargetOperators

Target.initEnvironment ()

let srcDir = "src"
let testDir = "tests"
let exampleDir = "examples"

Target.create "Clean" (fun _ ->
    !! (srcDir @@ "**/bin")
    ++ (srcDir @@ "**/obj")
    ++ (testDir @@ "**/bin")
    ++ (testDir @@ "**/obj")
    ++ (exampleDir @@ "**/bin")
    ++ (exampleDir @@ "**/obj")
    |> Shell.cleanDirs 
)

Target.create "BuildLibrary" (fun _ ->
    !! (srcDir @@ "**/*.*proj")
    |> Seq.iter (DotNet.build id)
)

Target.create "BuildTests" (fun _ ->
    !! (testDir @@ "**/*.*proj")
    |> Seq.iter (DotNet.build id)
)

Target.create "BuildExamples" (fun _ ->
    !! (exampleDir @@ "**/*.*proj")
    |> Seq.iter (DotNet.build id)
)

Target.create "Test" (fun _ ->
    !! (testDir @@ "**/*.fsproj")
    |> Seq.iter (DotNet.test id)
)

Target.create "All" ignore

"Clean"
  ==> "BuildLibrary"
  ==> "BuildTests"
  ==> "BuildExamples"
  ==> "Test"
  ==> "All"

Target.runOrDefault "All"
