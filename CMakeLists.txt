cmake_minimum_required(VERSION 3.8)
cmake_policy(VERSION 3.8)

project(jagged_lua LANGUAGES CSharp C CXX)

set(CMAKE_ARCHIVE_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/bin")
set(CMAKE_LIBRARY_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/bin")
set(CMAKE_RUNTIME_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/bin")

add_subdirectory(lua_core)
add_subdirectory(lua_interface)
add_subdirectory(test)
