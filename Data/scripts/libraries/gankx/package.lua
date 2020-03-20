Booter.packageDefine = {
    {
        name = "base",

        "string_ex.lua",
        "table_ex.lua",
        "util.lua",
        "bit_util.lua",
        "list.lua",
        "set.lua",
        "deque.lua",
        "array.lua",
        "steady_array.lua",
        "steady_order_array.lua",
        "string_builder.lua",
        "time_control.lua",
        "class.lua",
        "steady_list.lua",
        "reusing_ids.lua",
        "json.lua",
        "mathf.lua",
        "env_util.lua",
        "vector2.lua",
        "vector3.lua",
        "uft8_support.lua",
        "function_profiler.lua",
        "array_pool.lua",
    },

    {
        name = "unity",

        "async_operation.lua",
        "resources.lua",

        {
            name = "scene_management",

            "load_scene_mode.lua",
            "scene_manager.lua",
        }
    },

    {
        name = "diagnostics",

        "stopwatch.lua",
        "profiler.lua"
    },

    {
        name = "io",

        "path.lua",
        "file.lua",
        "directory.lua",
        "console.lua",

    },

    {
        name = "event",

        "event_listener.lua",
        "event_service.lua",
        "event.lua",
        "timer.lua",
        "system.lua",
    },

    {
        name = "coroutine",

        "yield_instruction.lua",
        "wait_for_seconds.lua",
        "async_operation.lua",
        "wait_for_event.lua",
        "wait_for_event_or_seconds.lua",
        "coroutine.lua",
        "composited_async_operation.lua",
    },

    {
        name = "game_object",

        "component.lua",
        "component_manager.lua",
        "game_object.lua",
    },

    {
        name = "application",

        "application.lua",
        "services.lua",
    },


    {
        name = "protobuf",
        "protobuf_library.lua",
    },

    {
        name = "network",
        "reconnect_state.lua",
        "network_protocol.lua",
        "network_platform.lua",
        "network_scope.lua",
        "network_state.lua",
        "network_result.lua",
        "network_send_type.lua",
        "network_resend_list.lua",
        "network_service.lua",
        "network_utility.lua",
        "network_talker.lua",
        "network_account.lua",

        "reconnector.lua",
        "reconnect_service.lua",
    },

    {
        name = "ui",

        {
            name = "base",
            "load_status.lua",
            "load_operation.lua",
            "export_wrapper.lua",
            "widget_handler.lua",
            "message_router.lua",
            "widget.lua",
            "panel_create_load_request.lua",
            "panel.lua",
            "panel_service.lua",
            "panel_unload_service.lua",
            "screen.lua",
            "widget_event_service.lua",
        },

        {
            name = "widget", 
            package = true
        },
    },

    {
        name = "system",

        "system_scope.lua",
        "system_component.lua",
        "system_service.lua",
    },

    {
        name = "clipboard",

        "clipboard_service.lua",
    },
}
