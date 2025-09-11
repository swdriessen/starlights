import { Label } from "@/components/ui/label";
import { Select, SelectContent, SelectGroup, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import type { CharacterCreationOptions } from "@/lib/api/characters/queries";

export function CharacterCreationOptionsSelect({
  characterCreationOptions,
  onValueChange,
}: {
  characterCreationOptions: CharacterCreationOptions | undefined;
  onValueChange?: (value: string) => void;
}) {
  return (
    <>
      <Label htmlFor="charactername" className="">
        Creation Option
      </Label>
      <Select onValueChange={onValueChange}>
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
    </>
  );
}
