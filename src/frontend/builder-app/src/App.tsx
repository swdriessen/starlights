import { Link, Outlet } from "react-router-dom";
import { AudioLines } from "lucide-react";
import { ModeToggle } from "./components/mode-toggle";
import { GitHubIconButton } from "./components/navigation/github-icon-button";
import "./App.css";

function App() {
  return (
    <div className="container mx-auto p-4">
      <header className="flex items-center justify-between gap-2">
        <nav className="flex items-center gap-3 text-sm">
          <Link to="/" className=" flex items-center gap-2 mr-4 lg:mr-10">
            <AudioLines className="h-6 w-6 mr-1 text-primary" />
            Project Starlights
          </Link>
          <Link to="/characters" className="hover:underline">
            Characters
          </Link>
          <Link to="/characters/create" className="hover:underline">
            Create Character
          </Link>
          <Link to="/about" className="hover:underline">
            About
          </Link>
        </nav>
        <div className="flex items-center justify-end gap-2">
          <ModeToggle />
          <GitHubIconButton />
        </div>
      </header>
      <hr className="my-4" />
      <main className="py-2">
        <Outlet />
      </main>
    </div>
  );
}

export default App;
