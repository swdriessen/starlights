import { useCharacterCreationOptions, useCharacterPortraitOptions } from "@/lib/api/characters/queries";
import { CharacterCreationOptionsSelect } from "./character-creation-options-select";
import { CharacterPortraitOptionsSelect } from "./character-creation-portraits-select";
import { useState } from "react";
import { BoxSelectIcon, Check, LoaderCircleIcon, OctagonAlert } from "lucide-react";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";

function CharacterCreation() {
  const { data: options, isLoading: optionsLoading, isError: optionsIsError, error: optionsError } = useCharacterCreationOptions();
  const { data: portraits, isLoading: portraitsLoading, isError: portraitsIsError, error: portraitsError } = useCharacterPortraitOptions();

  const [selectedOption, setSelectedOption] = useState<string | null>(null);
  const [selectedPortrait, setSelectedPortrait] = useState<string | null>(null);

  return (
    <div className="space-y-4">
      <div className="">
        {optionsLoading && <p>Loading Options...</p>}
        {optionsIsError && <p>Error: {optionsError.message}</p>}
        {options && <CharacterCreationOptionsSelect characterCreationOptions={options} onValueChange={setSelectedOption} />}
      </div>
      <div className="">
        {portraitsLoading && <p>Loading Portraits...</p>}
        {portraitsIsError && <p>Error: {portraitsError.message}</p>}
        <div className="grid grid-cols-12 gap-2">
          {portraits &&
            portraits.portraits.map((portrait) => (
              <div className="overflow-hidden rounded relative" key={portrait.url} onClick={() => setSelectedPortrait(portrait.url)}>
                <img className="w-full h-full object-cover" key={portrait.url} src={portrait.url} alt={portrait.description} title={portrait.description} />
                {selectedPortrait === portrait.url && (
                  <div className="absolute inset-0 bg-black/30 flex items-center justify-center">
                    <Check className="text-white" />
                  </div>
                )}
              </div>
            ))}
        </div>
      </div>
      <div>
        <Input name="characterName" placeholder="Character Name" />
      </div>
      <div>
        <Button type="submit">Create Character</Button>
      </div>
    </div>
  );
}
export default function CharactersCreatePage() {
  return (
    <>
      <div className="space-y-2">
        <h2 className="text-xl font-semibold">Characters Create</h2>
        <p>This is the Characters Create page for the Starlights builder app.</p>
      </div>
      <hr className="my-4" />

      <div className="space-y-4">
        <CharacterCreation />
      </div>
    </>
  );
}
