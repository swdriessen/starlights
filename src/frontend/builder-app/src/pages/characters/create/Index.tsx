import { useCharacterCreationOptions, useCharacterPortraitOptions, useCreateCharacter } from "@/lib/api/characters/queries";
import { CharacterCreationOptionsSelect } from "./character-creation-options-select";
// import { CharacterPortraitOptionsSelect } from "./character-creation-portraits-select";
import { useMemo } from "react";
import { Check } from "lucide-react";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { useNavigate } from "react-router-dom";
import { useForm } from "react-hook-form";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";

const schema = z.object({
  CharacterCreationOptionId: z.string().min(1, "Please select an option"),
  Name: z.string().trim().min(1, "Please enter a name"),
  PortraitUrl: z.string().url({ message: "Portrait must be a valid URL" }).optional(),
});

type FormValues = z.infer<typeof schema>;

function CharacterCreation() {
  const { data: options, isLoading: optionsLoading, isError: optionsIsError, error: optionsError } = useCharacterCreationOptions();
  const { data: portraits, isLoading: portraitsLoading, isError: portraitsIsError, error: portraitsError } = useCharacterPortraitOptions();
  const navigate = useNavigate();
  const createMutation = useCreateCharacter();

  const {
    register,
    handleSubmit,
    setValue,
    formState: { errors, isValid, isSubmitting },
    watch,
  } = useForm<FormValues>({
    resolver: zodResolver(schema),
    mode: "onChange",
    defaultValues: {
      CharacterCreationOptionId: "",
      Name: "",
      PortraitUrl: undefined,
    },
  });

  const selectedPortrait = watch("PortraitUrl");
  const canSubmit = useMemo(() => isValid && !createMutation.isPending && !isSubmitting, [isValid, createMutation.isPending, isSubmitting]);

  const onSubmit = handleSubmit(async (values) => {
    const result = await createMutation.mutateAsync(values);
    console.log(result);
    if (result?.Id) navigate(`/characters/${result.Id}`);
  });

  return (
    <form className="space-y-4" onSubmit={onSubmit}>
      <div className="">
        {optionsLoading && <p>Loading Options...</p>}
        {optionsIsError && <p>Error: {optionsError.message}</p>}
        {options && (
          <>
            <CharacterCreationOptionsSelect
              characterCreationOptions={options}
              onValueChange={(v) => setValue("CharacterCreationOptionId", v, { shouldValidate: true })}
            />
            {errors.CharacterCreationOptionId && <p className="text-sm text-red-600 mt-1">{errors.CharacterCreationOptionId.message}</p>}
          </>
        )}
      </div>
      <div className="">
        {portraitsLoading && <p>Loading Portraits...</p>}
        {portraitsIsError && <p>Error: {portraitsError.message}</p>}
        <div className="grid grid-cols-12 gap-2">
          {portraits &&
            portraits.portraits.map((portrait, index) => (
              <div
                className="overflow-hidden rounded relative cursor-pointer"
                key={index}
                onClick={() => setValue("PortraitUrl", portrait.url, { shouldValidate: true })}
              >
                <img className="w-full h-full object-cover" src={portrait.url} alt={portrait.description} title={portrait.description} />
                {selectedPortrait === portrait.url && (
                  <div className="absolute inset-0 bg-black/30 flex items-center justify-center">
                    <Check className="text-white" />
                  </div>
                )}
              </div>
            ))}
        </div>
        {errors.PortraitUrl && <p className="text-sm text-red-600 mt-1">{errors.PortraitUrl.message}</p>}
      </div>
      <div>
        <Input placeholder="Character Name" {...register("Name")} />
        {errors.Name && <p className="text-sm text-red-600 mt-1">{errors.Name.message}</p>}
      </div>
      <div className="flex items-center gap-3">
        <Button type="submit" disabled={!canSubmit}>
          {createMutation.isPending || isSubmitting ? "Creating..." : "Create Character"}
        </Button>
        {createMutation.isError && <p className="text-sm text-red-600">{createMutation.error?.message}</p>}
      </div>
    </form>
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
