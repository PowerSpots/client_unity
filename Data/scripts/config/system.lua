module("Gankx.Config", package.seeall)

------------------------------------------------------
-- 以下是域的包含关系，只有叶子节点上的域是可以被Switch的
-- 在进入任何叶节点的域时，其祖先节点的域都被启用了
-- 在退出的时候，相应的也会退出祖先域
------------------------------------------------------
------------------- Global
------------------- /    \
--------------- Login    Game
----------------------- /    \
--------------------- Hall   Battle
------------------------------------------------------
--- 会根据SystemScopeDefine生成定义到SystemScope
--- 在定义System时可以通过SystemScope直接访问

System = {
    define = {

        name = "Global",

        subDefines = {
            -- Login
            {
                name = "Login"
            },
            -- Game
            {
                name = "Game",
                subDefines = {
                    {
                        name = "Hall"
                    },
                    {
                        name = "Battle"
                    }
                }
            }
        }
    }
}