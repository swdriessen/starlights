import { Link, Outlet } from "react-router-dom";
import { ModeToggle } from "./components/mode-toggle";
import "./App.css";

function App() {
  return (
    <div className="container mx-auto p-4">
      <header className="flex items-center justify-between gap-2">
        <nav className="flex items-center gap-3 text-sm">
          <Link to="/" className="hover:underline">
            Home
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
        <ModeToggle />
      </header>
      <hr className="my-4" />
      <main className="py-2">
        <Outlet />
      </main>
    </div>
  );
}

export default App;
