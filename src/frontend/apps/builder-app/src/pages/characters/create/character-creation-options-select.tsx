import { Select, SelectContent, SelectGroup, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import type { CharacterCreationOptions } from "@/lib/api/characters/queries";
import { useEffect } from "react";

export function CharacterCreationOptionsSelect({
  characterCreationOptions,
  onValueChange,
}: {
  characterCreationOptions: CharacterCreationOptions | undefined;
  onValueChange?: (value: string) => void;
}) {
  const defaultValue = characterCreationOptions?.options[0]?.id;

  useEffect(() => {
    if (defaultValue && onValueChange) {
      onValueChange(defaultValue);
    }
  }, [defaultValue, onValueChange]);
  return (
    <>
      <Select onValueChange={onValueChange} defaultValue={defaultValue}>
        <SelectTrigger className="w-full">
          <SelectValue placeholder="Select your creation option" />
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
