// Agent.ts
import { ConfigManager } from "./ConfigManager";
import { Memory } from "./Memory";
import { Tools } from "./Tools";
import { TaskParser } from "./TaskParser";

export class Agent {
  private config: ConfigManager;
  private memory: Memory;
  private tools: Tools;
  private parser: TaskParser;

  constructor() {
    this.config = new ConfigManager();
    this.memory = new Memory();
    this.tools = new Tools();
    this.parser = new TaskParser();
  }

  handle(userInput: string): string {
    const task = this.parser.parse(userInput);
    let result: string;

    switch (task.type) {
      case "search":
        result = this.tools.searchWeb(task.query);
        break;
      case "email":
        result = this.tools.sendEmail(task.to, task.subject, task.body);
        break;
      default:
        result = `回覆: ${task.text}`;
    }

    this.memory.add(userInput, result);
    return result;
  }
}
