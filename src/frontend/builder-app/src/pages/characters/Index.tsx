import { useCharacterCards } from "@/lib/api/characters/queries";
import { Link } from "react-router-dom";

function CharacterCollection() {
  const { data: characterCards } = useCharacterCards();

  return (
    <div className="">
      {characterCards && (
        <ul className="space-y-2">
          {characterCards.characters.map((card) => (
            <li className="bg-accent/20 px-2 py-2 rounded" key={card.characterId}>
              <Link to={`/characters/${card.characterId}`} className="text-sm">
                <span className="font-semibold">{card.name}</span> | <span className="font-light text-muted-foreground text-xs">{card.characterId}</span>
              </Link>
              {card.portraitUrl && <img src={card.portraitUrl} alt={card.name} className="mt-1 w-full rounded" />}
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}

export default function CharactersPage() {
  return (
    <>
      <div className="space-y-2">
        <h2 className="text-xl font-semibold">Characters</h2>
        <p>This is the Characters page for the Starlights builder app.</p>
      </div>

      <hr className="my-4" />

      <CharacterCollection />
    </>
  );
}
