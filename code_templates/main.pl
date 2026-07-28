task(N) :- N =< 5, format("任務 ~w 完成！~n", [N]), N1 is N+1, task(N1).
task(N) :- N > 5.

:- task(1).
