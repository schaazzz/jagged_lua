file(GLOB lua_core src/*.c src/*.h src/lua.def)
source_group(lua_core FILES ${lua_core})

include_directories(src)

set (CMAKE_C_FLAGS "${CMAKE_C_FLAGS} /Gz")
set(CMAKE_SHARED_LINKER_FLAGS "${CMAKE_SHARED_LINKER_FLAGS} /DEF:${CMAKE_CURRENT_SOURCE_DIR}/src/lua.def")
add_library(lua_core SHARED ${lua_core})

set_source_files_properties(src/lua.def PROPERTIES HEADER_FILE_ONLY TRUE)
