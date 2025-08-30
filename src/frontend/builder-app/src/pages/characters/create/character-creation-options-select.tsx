import { Select, SelectContent, SelectGroup, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import type { CharacterCreationOptions, CharacterPortraitOptions } from "@/lib/api/characters/queries";

export function CharacterCreationOptionsSelect({ characterCreationOptions }: { characterCreationOptions: CharacterCreationOptions | undefined }) {
  return (
    <Select>
      <SelectTrigger className="w-full">
        <SelectValue placeholder="Select a character creation option" />
      </SelectTrigger>
      <SelectContent>
        <SelectGroup>
          {characterCreationOptions?.options.map((option) => (
            <SelectItem key={option.id} value={option.id}>
              {option.name}
            </SelectItem>
          ))}
        </SelectGroup>
      </SelectContent>
    </Select>
  );
}

export function CharacterPortraitOptionsSelect({ characterPortraitOptions }: { characterPortraitOptions: CharacterPortraitOptions | undefined }) {
  return (
    <Select>
      <SelectTrigger className="w-full">
        <SelectValue placeholder="Select a character portrait option" />
      </SelectTrigger>
      <SelectContent>
        <SelectGroup>
          {characterPortraitOptions?.portraits.map((option) => (
            <SelectItem key={option.url} value={option.url}>
              {option.description}
            </SelectItem>
          ))}
        </SelectGroup>
      </SelectContent>
    </Select>
  );
}
