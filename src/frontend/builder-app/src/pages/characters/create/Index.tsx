import { useCharacterCreationOptions, useCharacterPortraitOptions, useCreateCharacter } from "@/lib/api/characters/queries";
import { CharacterCreationOptionsSelect } from "./character-creation-options-select";
import { useMemo } from "react";
import { Check, CheckCheckIcon, CheckCircleIcon, CheckIcon, OctagonAlertIcon, RefreshCcwIcon } from "lucide-react";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { useNavigate } from "react-router-dom";
import { useForm } from "react-hook-form";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import { Label } from "@/components/ui/label";
import ProseSection from "@/components/prose-section";
import { Field, FieldContent, FieldDescription, FieldGroup, FieldLabel, FieldLegend, FieldSeparator, FieldSet } from "@/components/ui/field";
import { Card, CardContent } from "@/components/ui/card";
import { ScrollArea } from "@/components/ui/scroll-area";
import { Spinner } from "@/components/ui/spinner";
import { Empty, EmptyContent, EmptyDescription, EmptyHeader, EmptyMedia, EmptyTitle } from "@/components/ui/empty";
import { cn } from "@/lib/utils";

function PortraitsLoading() {
  return (
    <Empty className="size-full">
      <EmptyHeader>
        <EmptyMedia variant="icon">
          <Spinner />
        </EmptyMedia>
        <EmptyDescription>The portraits are loading.</EmptyDescription>
      </EmptyHeader>
    </Empty>
  );
}

function PortraitsError({ errorMessage }: { errorMessage: string }) {
  return (
    <Empty className="">
      <EmptyHeader>
        <EmptyMedia variant="icon">
          <OctagonAlertIcon className="text-destructive" />
        </EmptyMedia>
        <EmptyDescription>{errorMessage}</EmptyDescription>
      </EmptyHeader>
    </Empty>
  );
}
const schema = z.object({
  CharacterCreationOptionId: z.string().min(1, "Please select an option"),
  Name: z.string().trim().min(1, "Please enter a name"),
  PortraitUrl: z.string().optional(),
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
    <form className="flex flex-col gap-6" onSubmit={onSubmit}>
      <FieldSet>
        <FieldLegend>Character</FieldLegend>
        <FieldDescription>Fill in your character information. You can change all these fields later.</FieldDescription>
        <FieldSeparator />
        <FieldGroup>
          <Field orientation="responsive">
            <FieldContent>
              <FieldLabel>Creation Option</FieldLabel>
              <FieldDescription>
                This option determines your character's starting abilities and traits, this can vary based on the system or campaign you are playing.
              </FieldDescription>
            </FieldContent>
            <div className="md:min-w-80">
              {optionsLoading && <Spinner className="m-2" />}
              {optionsIsError && (
                <p className="text-sm flex items-center gap-2">
                  <OctagonAlertIcon className="text-destructive" /> Failed to load character options: {optionsError.message}
                </p>
              )}
              {options && (
                <>
                  <CharacterCreationOptionsSelect
                    characterCreationOptions={options}
                    onValueChange={(v) => setValue("CharacterCreationOptionId", v, { shouldValidate: false })}
                  />
                  {errors.CharacterCreationOptionId && <p className="text-sm text-red-600 mt-1">{errors.CharacterCreationOptionId.message}</p>}
                </>
              )}
            </div>
          </Field>
        </FieldGroup>
        <FieldSeparator />
        <FieldGroup>
          <Field orientation="responsive">
            <FieldContent>
              <FieldLabel htmlFor="charactername">Name</FieldLabel>
              <FieldDescription>Choose a unique name for your character.</FieldDescription>
            </FieldContent>
            <Input id="charactername" type="text" placeholder="The Nameless One" required {...register("Name")} className="md:min-w-80" />
          </Field>
        </FieldGroup>
        <FieldSeparator />
        <Field>
          <FieldContent>
            <FieldLabel>Portrait</FieldLabel>
            <FieldDescription>Select a portrait for your character.</FieldDescription>
          </FieldContent>
          <ScrollArea className="h-60 rounded-md border border-dashed whitespace-nowrap">
            <div className="p-2">
              {portraitsLoading && <PortraitsLoading />}
              {portraitsIsError && <PortraitsError errorMessage={portraitsError.message} />}
              <div className="grid grid-cols-4 sm:grid-cols-4 md:grid-cols-7 lg:grid-cols-8 xl:grid-cols-12 gap-2">
                {portraits &&
                  portraits.portraits.map((portrait, index) => (
                    <div
                      className={cn("overflow-hidden rounded relative hover:ring-2 hover:ring-tertiary transition-all", {
                        "ring-2 ring-tertiary": selectedPortrait === portrait.url,
                      })}
                      key={index}
                      onClick={() => setValue("PortraitUrl", portrait.url, { shouldValidate: true })}
                    >
                      <img className="size-full aspect-square object-cover" src={portrait.url} alt={portrait.description} title={portrait.description} />
                      {selectedPortrait === portrait.url && (
                        <div className="absolute inset-0 bg-black/30 flex items-center justify-center">
                          <CheckIcon className="text-tertiary-200" size={36} />
                        </div>
                      )}
                    </div>
                  ))}
              </div>
              {errors.PortraitUrl && <p className="text-sm text-red-600 mt-1">{errors.PortraitUrl.message}</p>}
            </div>
          </ScrollArea>
        </Field>
        <FieldSeparator />
      </FieldSet>

      <div className="flex items-center justify-end gap-3">
        <Button type="submit" variant="default" disabled={!canSubmit} className="w-full sm:w-auto">
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
      <div className="flex-row md:flex gap-2">
        <ProseSection className="flex-grow">
          <h1 className="mb-0">Create New Character</h1>
          <p className="mt-0">Use the form below to create a new character for your adventures.</p>
        </ProseSection>
      </div>

      <hr className="my-4" />

      <Card className="mb-4">
        <CardContent>
          <CharacterCreation />
        </CardContent>
      </Card>
    </>
  );
}
