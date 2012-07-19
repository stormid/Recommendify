solution_file = "Recommendify.sln"
configuration = "release"
test_assemblies = "src/Recommendify.Specs/bin/${configuration}/Recommendify.Specs.dll"

target default, (package):
	pass

desc "Compiles solution"	
target compile:
	msbuild(file: solution_file, configuration: configuration, version: "4.0")

desc "Executes unit tests"
target test, (compile):
	mspec(assembly: test_assemblies, toolPath: "packages/Machine.Specifications.0.5.7/tools")

desc "Copies binaries to the build directory"
target deploy:
	rm('build')

	with FileList():
		.Include("src/**/bin/${configuration}/Recommendify.*")
		.Exclude("src/**/bin/${configuration}/Recommendify.Specs.*")
		.Include("License.txt")
		.Include("readme.md")
		.Flatten(true)
		.ForEach def(file):
			file.CopyToDirectory("build/${configuration}")

desc "Creates zip and nuget packages"
target package, (test, deploy):
	zip("build/${configuration}", "build/Recommendify.zip")

	nuget_pack(toolPath: ".nuget/nuget.exe", nuspecFile: "recommendify.nuspec", outputDirectory: "build/nuget")

desc "Publishes nuget package"
target publish, (package):
	apiKey = env("apiKey")

	with FileList():
		.Include("build/nuget/*.nupkg")
		.Flatten(true)
		.ForEach def(file):
			exec("echo publishing ${file.FullName}")
			nuget_push(toolPath: ".nuget/nuget.exe", apiKey: apiKey, packagePath: file.FullName)