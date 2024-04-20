# ZmqPb

This is a library for C# to simplify the use of the networking library ZeroMQ, together with the use of protobuf messages.

## Example Project Layout

C# project file (`.csproj`)
```xml
<!-- This target builds the .proto files from a `protos` subfolder into C# classes located inside a `Messages` subfolder -->
<Target Name="BuildProto" AfterTargets="PreBuildEvent">
  <RemoveDir Condition="Exists('$(ProjectDir)/Messages/')" Directories="$(ProjectDir)/Messages/" />
  <MakeDir Directories="$(ProjectDir)/Messages/" />
  <Exec WorkingDirectory="./" Command="$(NugetPackageRoot)/google.protobuf.tools/$(ProtoBufToolsVersion)/tools/$(ProtoBufToolsOS)_$(ProtoBufTools64)/protoc.exe -I=&quot;$(ProjectDir)/protos/&quot; --csharp_out=&quot;$(ProjectDir)/Messages/&quot; &quot;$(ProjectDir)/protos/*.proto&quot;" />
</Target>
...
<!-- The direct ZmqPb dependency -->
<ItemGroup>
    <Reference Include="ZmqPb">
      <HintPath>$(Path_To_ZmqPb.dll)/ZmqPb.dll</HintPath>
    </Reference>
</ItemGroup>
<!-- Dependencies for compiling -->
<ItemGroup>
  <PackageReference Include="Google.Protobuf" Version="$(ProtoBufToolsVersion)" />
  <PackageReference Include="Google.Protobuf.Tools" Version="$(ProtoBufToolsVersion)" />
  <PackageReference Include="ZeroMQ" Version="4.1.0.31" />
</ItemGroup>
<!-- The `protos` and `Messages` subfolders from above -->
<ItemGroup>
  <Folder Include="Messages/" />
  <Folder Include="protos/" />
</ItemGroup>
```
