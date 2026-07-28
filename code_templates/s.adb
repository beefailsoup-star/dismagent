with Ada.Text_IO; use Ada.Text_IO;

procedure Main is
begin
   for I in 1 .. 5 loop
      Put_Line("任務 " & Integer'Image(I) & " 完成！");
   end loop;
end Main;
