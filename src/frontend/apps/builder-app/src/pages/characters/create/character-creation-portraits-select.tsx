import { Select, SelectContent, SelectGroup, SelectItem, SelectTrigger, SelectValue } from "@starlights/ui-framework/select";
import type { CharacterPortraitOption, CharacterPortraitOptions } from "@/lib/api/characters/queries";

export function CharacterPortraitOptionsSelect({
  characterPortraitOptions,
  onValueChange,
}: {
  characterPortraitOptions: CharacterPortraitOptions | undefined;
  onValueChange?: (value: string | null) => void;
}) {
  return (
    <Select onValueChange={onValueChange}>
      <SelectTrigger className="w-full">
        <SelectValue placeholder="Select a character portrait option" />
      </SelectTrigger>
      <SelectContent>
        <SelectGroup>
          {characterPortraitOptions?.portraits.map((option: CharacterPortraitOption) => (
            <SelectItem key={option.url} value={option.url}>
              {option.description}
            </SelectItem>
          ))}
        </SelectGroup>
      </SelectContent>
    </Select>
  );
}
