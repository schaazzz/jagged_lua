file(GLOB lua_interface *.cs)
source_group(lua_interface FILES ${lua_interface})

add_library(lua_interface ${lua_interface})

target_compile_options(lua_interface PRIVATE "/langversion:6")

set_target_properties(lua_interface PROPERTIES VS_DOTNET_TARGET_FRAMEWORK_VERSION "v4.5.2")
