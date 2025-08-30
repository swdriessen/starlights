import { useCharacterCreationOptions, useCharacterPortraitOptions } from "@/lib/api/characters/queries";
import { CharacterCreationOptionsSelect, CharacterPortraitOptionsSelect } from "./create/character-creation-options-select";

function CharacterCollection() {
  return (
    <div className="">
      <p>Character Collection</p>
    </div>
  );
}

export default function CharactersPage() {
  const { data: options, isLoading: optionsLoading, isError: optionsIsError, error: optionsError } = useCharacterCreationOptions();
  const { data: portraits, isLoading: portraitsLoading, isError: portraitsIsError, error: portraitsError } = useCharacterPortraitOptions();

  return (
    <>
      <div className="space-y-2">
        <h2 className="text-xl font-semibold">Characters</h2>
        <p>This is the Characters page for the Starlights builder app.</p>
      </div>

      <hr className="my-4" />

      <div className="space-y-2">
        {optionsLoading && <p>Loading Options...</p>}
        {optionsIsError && <p>Error: {optionsError.message}</p>}
        {options && <CharacterCreationOptionsSelect characterCreationOptions={options} />}
      </div>

      <hr className="my-4" />

      <div className="space-y-2">
        {portraitsLoading && <p>Loading Portraits...</p>}
        {portraitsIsError && <p>Error: {portraitsError.message}</p>}
        {portraits && <CharacterPortraitOptionsSelect characterPortraitOptions={portraits} />}
      </div>

      <hr className="my-4" />

      <CharacterCollection />
    </>
  );
}
