import subprocess
import sys
import shutil
import re

MODEL = sys.argv[1] if len(sys.argv) > 1 else "llama3"

def show_progress(line: str):
    m = re.search(r"(\d+)%.*?(\d+\.?\d*)\s*(GB|MB|KB)/s.*?(\d+)s", line)
    if m:
        pct, speed_val, unit, eta = m.group(1), m.group(2), m.group(3), m.group(4)
        speed = f"{speed_val} {unit}/s"
        bar_width = 30
        filled = int(pct) * bar_width // 100
        bar = "\u2588" * filled + " " * (bar_width - filled)
        print(f"\r  Pulling... {pct}% \u2595{bar}\u258f {speed}  ETA: {eta}s  ", end="", flush=True)
    elif any(kw in line for kw in ("pulling manifest", "verifying", "writing manifest", "success")):
        print(f"\r{line}")
    elif line.strip():
        print(f"\r{line}")

def main():
    print("\n  Ollama Model Downloader\n")
    print(f"  Model: {MODEL}\n")

    if not shutil.which("ollama"):
        print("  [!] Ollama not found. Install from https://ollama.ai")
        input("\n  Press Enter to exit...")
        sys.exit(1)

    confirm = input(f"  Pull model '{MODEL}'? (Y/n): ").strip().lower()
    if confirm == "n":
        return

    print()
    proc = subprocess.Popen(
        ["ollama", "pull", MODEL],
        stdout=subprocess.PIPE,
        stderr=subprocess.STDOUT,
        text=True,
        bufsize=1,
    )
    for line in proc.stdout or []:
        show_progress(line)
    proc.wait()

    if proc.returncode == 0:
        print("\n  \u2713 Download complete")
    else:
        print("\n  [!] Download failed")

    input("\n  Press Enter to exit...")

if __name__ == "__main__":
    main()
