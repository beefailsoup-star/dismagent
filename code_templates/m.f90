-module(main).
-export([start/0]).

start() ->
    lists:foreach(fun(I) ->
        io:format("任務 ~p 完成！~n", [I])
    end, lists:seq(1,5)).
