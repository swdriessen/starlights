import { useCharacterCards, useDeleteCharacter } from "@/lib/api/characters/queries";
import { Link } from "react-router-dom";
import { toast } from "sonner";

function CharacterCollection() {
  const { data: characterCards } = useCharacterCards();
  const deleteCharacter = useDeleteCharacter();

  return (
    <div className="">
      {characterCards && (
        <ul className="space-y-2">
          {characterCards.characters.map((card) => (
            <li className="bg-accent/20 px-2 py-2 rounded" key={card.characterId}>
              <div className="flex items-start justify-between gap-2">
                <Link to={`/characters/${card.characterId}`} className="text-sm">
                  <span className="font-semibold">{card.name}</span> | <span className="font-light text-muted-foreground text-xs">{card.characterId}</span>
                </Link>
                <button
                  type="button"
                  className="text-red-600 text-xs hover:underline disabled:opacity-50"
                  onClick={() =>
                    deleteCharacter.mutate(card.characterId, {
                      onSuccess: () => toast.success("Character deleted"),
                      onError: (err) => toast.error("Failed to delete character", { description: err.message }),
                    })
                  }
                  disabled={deleteCharacter.isPending}
                  title="Delete character"
                >
                  {deleteCharacter.isPending ? "Deleting…" : "Delete"}
                </button>
              </div>
              <br />

              {card.portraitUrl && <img src={card.portraitUrl} alt={card.name} className="mt-1 w-full rounded h-30 object-cover" />}
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
