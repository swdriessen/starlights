import { SpellComponentExample } from "./components/spell-example";

export function App() {
  return (
    <>
      <div>
        <h1 className="text-3xl font-bold underline">ComponentExample</h1>
      </div>
      <SpellComponentExample />
    </>
  );
}

export function App2() {
  return <SpellComponentExample />;
}
export default App;
