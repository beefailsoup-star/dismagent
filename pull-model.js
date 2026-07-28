const { execSync, spawn } = require("child_process");
const readline = require("readline");

const MODEL = process.argv[2] || "llama3";

function confirm(query) {
  const rl = readline.createInterface({ input: process.stdin, output: process.stdout });
  return new Promise((resolve) => rl.question(query, (a) => { rl.close(); resolve(a.trim().toLowerCase() !== "n"); }));
}

function showProgress(line) {
  const m = line.match(/(\d+)%.*?(\d+\.?\d*)\s*(GB|MB|KB)\/s.*?(\d+)s/);
  if (m) {
    const pct = m[1], speed = `${m[2]} ${m[3]}/s`, eta = m[4];
    const barWidth = 30;
    const filled = (parseInt(pct) * barWidth / 100) | 0;
    const bar = "\u2588".repeat(filled) + " ".repeat(barWidth - filled);
    process.stdout.write(`\r  Pulling... ${pct}% \u2595${bar}\u258f ${speed}  ETA: ${eta}s  `);
  } else if (/pulling manifest|verifying|writing manifest|success/.test(line)) {
    console.log(`\r${line}`);
  } else if (line.trim()) {
    console.log(`\r${line}`);
  }
}

async function main() {
  console.log(`\n  Ollama Model Downloader\n  Model: ${MODEL}\n`);

  try { execSync("where ollama", { stdio: "ignore" }); }
  catch {
    console.log("  [!] Ollama not found. Install from https://ollama.ai");
    return;
  }

  if (!(await confirm(`  Pull model '${MODEL}'? (Y/n): `))) return;

  console.log();
  const proc = spawn("ollama", ["pull", MODEL], { stdio: ["ignore", "pipe", "pipe"], shell: true });

  proc.stdout.on("data", (d) => d.toString().split("\n").forEach(showProgress));
  proc.stderr.on("data", (d) => d.toString().split("\n").forEach(showProgress));

  proc.on("exit", (code) => {
    console.log(code === 0 ? "\n  \u2713 Download complete" : "\n  [!] Download failed");
  });
}

main();
