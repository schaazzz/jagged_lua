file(GLOB test *.cs)
source_group(test FILES ${test})

add_executable(test_out ${test})

target_compile_options(test_out PRIVATE "/langversion:6")

target_link_libraries(test_out lua_interface)


set_target_properties(test_out PROPERTIES VS_DOTNET_REFERENCES "lua_interface")
set_target_properties(test_out PROPERTIES VS_DOTNET_TARGET_FRAMEWORK_VERSION "v4.5.2")
