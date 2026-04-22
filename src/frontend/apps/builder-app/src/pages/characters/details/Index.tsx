import { Button } from "@/components/ui/button";
import { Separator } from "@/components/ui/separator";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import {
  useCharacterClasses,
  useRegisterSelectionMutation,
  useRegistrationModels,
  useSelectionRuleDataModels,
  useSelectionRuleOptionModels,
  useStatistics,
  useUnregisterSelectionMutation,
  useUpdateClassLevelMutation,
  type CharacterClass,
} from "@/lib/api/builder/registration-api";
import {
  useAbilityScores,
  useCharacterDetails,
  useSavingThrows,
  useSkills,
  useUpdateAdditionalAbilityScore,
  useUpdateBaseAbilityScore,
} from "@/lib/api/characters/queries";
import { ChevronDown, ChevronUp } from "lucide-react";
import type { JSX } from "react";
import { useParams } from "react-router-dom";
import { CardWrapper } from "../components/card-wrapper";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";

function AbilitiesComponent({ id: characterId }: { id: string }) {
  const { data: abilityScores } = useAbilityScores(characterId);
  const updateBaseAbilityScore = useUpdateBaseAbilityScore(characterId);
  const updateAdditionalAbilityScore = useUpdateAdditionalAbilityScore(characterId);

  return (
    <>
      <div className="flex flex-row items-center justify-between mb-2 border border-dashed rounded p-4 ">
        {abilityScores && (
          <div className="overflow-x-auto text-sm">
            <table className="min-w-full text-center">
              <thead className="text-xs  uppercase font-semibold">
                <tr>
                  <th className="px-2 py-2 border-b text-left">Ability</th>
                  <th className="px-2 py-2 border-b">Score</th>
                  <th className="px-2 py-2 border-b">Modifier</th>
                  <th className="px-2 py-2 border-b">Base</th>
                  <th className="px-2 py-2 border-b">Additional</th>
                </tr>
              </thead>
              <tbody>
                {abilityScores.abilityScores.map((ability) => (
                  <tr key={ability.abilityScoreId} className="">
                    <td className="px-2 py-1 border-b ">
                      <span className="flex items-center gap-1 ">{ability.name}</span>
                    </td>
                    <td className="px-2 py-1 border-b text-sm">{ability.calculatedScore}</td>
                    <td className="px-2 py-1 border-b text-sm">
                      {ability.calculatedModifier != null
                        ? ability.calculatedModifier >= 0
                          ? `+${ability.calculatedModifier}`
                          : `${ability.calculatedModifier}`
                        : null}
                    </td>
                    <td className="px-2 py-1 border-b ">
                      <div className="flex flex-row items-center justify-center gap-2 ">
                        <Button
                          size="icon"
                          variant="outline"
                          onClick={() => {
                            const newScore = ability.baseScore - 1;
                            console.log(`Decrease ${ability.name} base score to ${newScore}`);
                            updateBaseAbilityScore.mutate({ abilityScoreId: ability.abilityScoreId, value: newScore });
                          }}
                          className="h-8 w-8 p-0"
                          title={`Decrease ${ability.name} base score`}
                          aria-label={`Decrease ${ability.name} base score`}
                        >
                          <ChevronDown />
                        </Button>

                        <span className="text-xs py-1 w-10 h-8 border border-dashed rounded leading-6 text-center">{ability.baseScore}</span>

                        <Button
                          size="icon"
                          variant="outline"
                          onClick={() => {
                            const newScore = ability.baseScore + 1;
                            console.log(`Increase ${ability.name} base score to ${newScore}`);
                            updateBaseAbilityScore.mutate({ abilityScoreId: ability.abilityScoreId, value: newScore });
                          }}
                          className="h-8 w-8 p-0"
                          title={`Increase ${ability.name} base score`}
                          aria-label={`Increase ${ability.name} base score`}
                        >
                          <ChevronUp />
                        </Button>
                      </div>
                    </td>
                    <td className="px-2 py-1 border-b">
                      <div className="flex flex-row items-center justify-center gap-2 ">
                        <Button
                          size="icon"
                          variant="outline"
                          onClick={() => {
                            const newScore = ability.additionalScore - 1;
                            console.log(`Decrease ${ability.name} additional score to ${newScore}`);
                            updateAdditionalAbilityScore.mutate({ abilityScoreId: ability.abilityScoreId, value: newScore });
                          }}
                          className="h-8 w-8 p-0 hidden"
                          title={`Decrease ${ability.name} additional score`}
                          aria-label={`Decrease ${ability.name} additional score`}
                        >
                          <ChevronDown />
                        </Button>

                        <span className="text-xs py-1 w-10 h-8 border border-dashed rounded leading-6 text-center">{ability.additionalScore}</span>

                        <Button
                          size="icon"
                          variant="outline"
                          onClick={() => {
                            const newScore = ability.additionalScore + 1;
                            console.log(`Increase ${ability.name} additional score to ${newScore}`);
                            updateAdditionalAbilityScore.mutate({ abilityScoreId: ability.abilityScoreId, value: newScore });
                          }}
                          className="h-8 w-8 p-0 hidden"
                          title={`Increase ${ability.name} additional score`}
                          aria-label={`Increase ${ability.name} additional score`}
                        >
                          <ChevronUp />
                        </Button>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </>
  );
}

function SavingThrowsComponent({ characterId }: { characterId: string }) {
  const { data: savingThrowsData, isLoading, error } = useSavingThrows(characterId);

  if (isLoading) return <div>Loading...</div>;
  if (error) return <div>Error: {error.message}</div>;

  return (
    <div className="flex flex-row items-center justify-between mb-2 border border-dashed rounded p-4 ">
      {savingThrowsData && (
        <div className="overflow-x-auto  text-sm">
          <table className="min-w-full text-center">
            <thead className="text-xs uppercase font-semibold">
              <tr>
                <th className="px-2 py-2 border-b text-left">Saving Throw</th>
                <th className="px-2 py-2 border-b">Bonus</th>
              </tr>
            </thead>
            <tbody>
              {savingThrowsData.savingThrows.map((save) => (
                <tr key={save.savingThrowId}>
                  <td className="px-2 py-1 border-b ">
                    <span className="flex items-center gap-1 ">
                      {save.name.replace("Saving Throw", "").trim()}
                      {/* <span className="text-muted-foreground text-xxs">({save.abilityScoreAbbreviation})</span> */}
                    </span>
                  </td>

                  <td className="px-2 py-1 border-b text-sm">
                    {save.calculatedBonus != null ? (save.calculatedBonus >= 0 ? `+${save.calculatedBonus}` : `${save.calculatedBonus}`) : null}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}

function SkillsComponent({ characterId }: { characterId: string }) {
  const { data: skillsData, isLoading, error } = useSkills(characterId);
  if (isLoading) return <div>Loading...</div>;
  if (error) return <div>Error: {error.message}</div>;
  return (
    <div className="flex flex-row items-center justify-between mb-2 border border-dashed rounded p-4 text-sm">
      {skillsData && (
        <div className="overflow-x-auto ">
          <table className="min-w-full text-center">
            <thead className="text-xs uppercase font-semibold">
              <tr>
                <th className="px-2 py-2 border-b text-left">Skill</th>
                <th className="px-2 py-2 border-b">Bonus</th>
              </tr>
            </thead>
            <tbody>
              {skillsData.skills.map((skill) => (
                <tr key={skill.skillId}>
                  <td className="px-2 py-1 border-b text-left">
                    <span className="flex items-center gap-1 text-sm">
                      {skill.name}
                      <span className="text-muted-foreground text-xxs">({skill.abilityScoreAbbreviation})</span>
                    </span>
                  </td>

                  <td className="px-2 py-1 border-b">
                    <div className="flex flex-row items-center justify-center gap-2 ">
                      <span className="text-center">
                        {skill.calculatedBonus != null ? (skill.calculatedBonus >= 0 ? `+${skill.calculatedBonus}` : `${skill.calculatedBonus}`) : null}
                      </span>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}

function SelectionRulesSectionOptionsComponent({
  characterId,
  selectionRuleId,
  parentRegistration,
}: {
  characterId: string;
  selectionRuleId: string;
  parentRegistration: string;
}) {
  const { data: optionsData, isLoading, error } = useSelectionRuleOptionModels(characterId, selectionRuleId);

  const registerSelectionMutation = useRegisterSelectionMutation(characterId, selectionRuleId);
  const unregisterSelectionMutation = useUnregisterSelectionMutation(characterId, selectionRuleId);

  if (isLoading) return <div>Loading...</div>;
  if (error) return <div>Error: {error.message}</div>;

  return (
    <div>
      {optionsData && (
        <>
          <div className="border border-dashed rounded p-4 my-2">
            <ul>
              {optionsData.options.map((option) => (
                <li key={option.elementId}>
                  {option.name} | ID: {option.elementId} |{" "}
                  <Button
                    size="sm"
                    disabled={registerSelectionMutation.isPending}
                    onClick={() => registerSelectionMutation.mutate({ parentRegistration: parentRegistration, elementId: option.elementId })}
                  >
                    Register
                  </Button>
                  <Button
                    size="sm"
                    variant="destructive"
                    disabled={unregisterSelectionMutation.isPending}
                    onClick={() => unregisterSelectionMutation.mutate({ parentRegistration: parentRegistration, elementId: option.elementId })}
                    className="ml-2"
                  >
                    Unregister
                  </Button>
                </li>
              ))}
            </ul>
          </div>
        </>
      )}
    </div>
  );
}

function SelectionRulesSectionComponent({
  characterId,
  type,
}: {
  characterId: string;
  type: "Class" | "Species" | "Background" | "SubClass" | "Proficiency" | "Language" | "Alignment";
}) {
  const { data: selectionRulesData, isLoading, error } = useSelectionRuleDataModels(characterId, type);

  if (isLoading) return <div>Loading...</div>;
  if (error) return <div>Error: {error.message}</div>;

  return (
    <div className="flex flex-row items-center justify-between mb-2 border border-dashed rounded p-4 ">
      {selectionRulesData && (
        <div className="overflow-x-auto  text-sm w-full">
          <h4>{type} Selection Rules</h4>
          {selectionRulesData.rules.map((rule) => (
            // 1 component per rule with its options and registration button

            <div key={rule.registrationSelectionRuleId}>
              <Separator className="my-4" />
              <div className="border border-dashed rounded p-4 my-2">
                <h5 className="text-sm font-semibold mb-1">
                  {rule.name} ({rule.type})
                </h5>
                <p className="text-xxs text-muted-foreground mb-2">
                  ID: {rule.registrationSelectionRuleId} | Registered: {rule.activeRegistration ? "Yes" : "No"}
                </p>

                <SelectionRulesSectionOptionsComponent
                  characterId={characterId}
                  selectionRuleId={rule.registrationSelectionRuleId}
                  parentRegistration={rule.registrationId}
                />
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}

function CharacterClassesComponent({ characterId }: { characterId: string }) {
  const { data: characterClassesData, isLoading, error } = useCharacterClasses(characterId);

  if (isLoading) return <div>Loading...</div>;
  if (error) return <div>Error: {error.message}</div>;

  return (
    <div>
      <h4>Character Classes</h4>
      <ul>
        {characterClassesData?.classes.map((characterClass) => (
          <li key={characterClass.characterClassId}>
            <CharacterClassComponent characterId={characterId} characterClass={characterClass} />
          </li>
        ))}
      </ul>
    </div>
  );
}

function CharacterClassComponent({ characterId, characterClass }: { characterId: string; characterClass: CharacterClass }) {
  const updateClassLevelMutation = useUpdateClassLevelMutation(characterId, characterClass.characterClassId);

  return (
    <>
      {characterClass.name} (Level {characterClass.level}){characterClass.isPrimary ? " [Primary]" : ""}
      <Button
        size="sm"
        className="ml-2"
        onClick={() => updateClassLevelMutation.mutate({ newLevel: characterClass.level + 1 })}
        disabled={updateClassLevelMutation.isPending || characterClass.level >= 20}
      >
        Level Up
      </Button>
      <Button
        size="sm"
        className="ml-2"
        onClick={() => updateClassLevelMutation.mutate({ newLevel: characterClass.level - 1 })}
        disabled={updateClassLevelMutation.isPending || characterClass.level <= 1}
      >
        Level Down
      </Button>
    </>
  );
}

export default function CharactersDetailsPage() {
  const { id } = useParams<{ id: string }>();

  if (!id) return <div>Character ID is required.</div>;
  const { data: characterDetails } = useCharacterDetails(id);
  const { data: registrationModels } = useRegistrationModels(id);
  const { data: statisticsData } = useStatistics(id);

  return (
    <>
      <div className="container mx-auto px-4 mt-12 mb-12">
        <CardWrapper>
          <Card className="rounded-lg">
            <CardHeader className="border-b">
              <CardTitle>Character Details</CardTitle>
              <CardDescription>
                This is a test page is for viewing and editing character details through the API. This is by no means representative of the final UI.
              </CardDescription>
            </CardHeader>
            <CardContent>
              <div className="flex flex-between gap-4">
                <dl className="grid grid-cols-2 gap-1 flex-1">
                  <dt>Id:</dt>
                  <dd>{id}</dd>
                  <dt>Name:</dt>
                  <dd>{characterDetails?.character.name ?? <span className="text-yellow-700">Loading...</span>}</dd>
                  <dt>Level:</dt>
                  <dd>{characterDetails?.character.level ?? <span className="text-yellow-700">Loading...</span>}</dd>
                  <dt>Build:</dt>
                  <dd>{characterDetails?.character.build ?? <span className="text-yellow-700">Loading...</span>}</dd>
                  <dt>Portrait URL:</dt>
                  <dd>{characterDetails?.character.portraitUrl ?? <span className="text-yellow-700">Loading...</span>}</dd>
                </dl>
                <div className="shrink-0">
                  <img src={characterDetails?.character.portraitUrl} alt="Character Portrait" className="h-32 w-32 object-cover mt-2 rounded" />
                </div>
              </div>

              <Separator className="my-4" />

              <div className="grid grid-cols-12 gap-2">
                <div className="col-span-12">
                  <h5>Character Sheet Data</h5>
                </div>
                <Tabs defaultValue="tab-sheet-abilities" className="col-span-12">
                  <TabsList className="">
                    <TabsTrigger value="tab-sheet-abilities">Abilities / Skills</TabsTrigger>
                    {/* <TabsTrigger value="tab-sheet-2">Saving Throws</TabsTrigger> */}
                    {/* <TabsTrigger value="tab-sheet-3">Skills</TabsTrigger> */}
                    <TabsTrigger value="tab-sheet-4">Class Features</TabsTrigger>
                    <TabsTrigger value="tab-statistics">Statistics</TabsTrigger>
                  </TabsList>
                  <TabsContent value="tab-sheet-abilities" className="flex flex-row items-start gap-4 ">
                    <AbilitiesComponent id={id} />
                    <SavingThrowsComponent characterId={id} />
                    <SkillsComponent characterId={id} />
                  </TabsContent>
                  <TabsContent value="tab-sheet-2">
                    <SavingThrowsComponent characterId={id} />
                  </TabsContent>
                  <TabsContent value="tab-sheet-3">
                    <SkillsComponent characterId={id} />
                  </TabsContent>
                  <TabsContent value="tab-sheet-4">
                    <>
                      {registrationModels ? (
                        <div className="border border-dashed rounded p-4 overflow-x-auto text-sm">
                          {(() => {
                            const renderNode = (reg: any, level = 0): JSX.Element => (
                              <div key={reg.registrationId} style={{ marginLeft: level * 16 }} className="mb-2">
                                <div className="font-bold">
                                  {reg.name} <span className="text-muted-foreground">({reg.type})</span>
                                </div>
                                {/* <div className="text-xxs text-muted-foreground">ID: {reg.registrationId}</div> */}
                                {reg.children && reg.children.length > 0 && <div>{reg.children.map((child: any) => renderNode(child, level + 1))}</div>}
                              </div>
                            );

                            return <div>{registrationModels.registrations.map((r: any) => renderNode(r, 0))}</div>;
                          })()}
                        </div>
                      ) : (
                        <span className="text-yellow-700">Loading...</span>
                      )}
                    </>
                  </TabsContent>
                  <TabsContent value="tab-statistics">
                    <>
                      {statisticsData ? (
                        <div className="border border-dashed rounded p-4 overflow-x-auto text-sm">
                          <table className="min-w-full text-center">
                            <thead className="text-xs uppercase font-semibold">
                              <tr>
                                <th className="px-2 py-2 border-b text-left">Statistic Group</th>
                                <th className="px-2 py-2 border-b">Total Value</th>
                                <th className="px-2 py-2 border-b">Finalized</th>
                                <th className="px-2 py-2 border-b text-left">Details</th>
                              </tr>
                            </thead>
                            <tbody>
                              {statisticsData.statistics.map((group) => (
                                <tr key={group.groupName}>
                                  <td className="px-2 py-1 border-b text-left font-semibold">{group.groupName}</td>
                                  <td className="px-2 py-1 border-b">{group.totalValue}</td>
                                  <td className="px-2 py-1 border-b">{group.isFinalized ? "Yes" : "No"}</td>
                                  <td className="px-2 py-1 border-b text-left">
                                    <div className="space-y-1">
                                      {group.values.map((value, index) => (
                                        <div key={`${value.source}-${index}`} className="text-xs">
                                          <span className="text-muted-foreground">{value.displayName || value.source}:</span>{" "}
                                          <span className="font-medium">{value.value}</span>
                                        </div>
                                      ))}
                                    </div>
                                  </td>
                                </tr>
                              ))}
                            </tbody>
                          </table>
                        </div>
                      ) : (
                        <span className="text-yellow-700">Loading...</span>
                      )}
                    </>
                  </TabsContent>
                </Tabs>
              </div>

              <Separator className="my-4" />

              <div className="grid grid-cols-12 gap-2">
                <div className="col-span-12">
                  <h5>Actions | Commands</h5>
                </div>
                <Tabs defaultValue="tab-actions-1" className="col-span-12">
                  <TabsList className="">
                    <TabsTrigger value="tab-actions-1">Class Details</TabsTrigger>
                  </TabsList>
                  <TabsContent value="tab-actions-1">
                    <div className="border border-dashed rounded p-4 my-2">
                      <CharacterClassesComponent characterId={id} />
                    </div>
                  </TabsContent>
                </Tabs>
              </div>
              <Separator className="my-4" />

              <div className="grid grid-cols-12 gap-2">
                <div className="col-span-12">
                  <h5>Build | Selection Rules</h5>
                </div>
                <Tabs defaultValue="tab-selection-rules-class" className="col-span-12">
                  <TabsList className="">
                    <TabsTrigger value="tab-selection-rules-class">Character Class</TabsTrigger>
                    <TabsTrigger value="tab-selection-rules-race">Character Origin</TabsTrigger>
                    {/* <TabsTrigger value="tab-selection-rules-background">Background</TabsTrigger> */}
                    <TabsTrigger value="tab-selection-rules-proficiency">Proficiency</TabsTrigger>
                    <TabsTrigger value="tab-selection-rules-language">Language</TabsTrigger>
                    <TabsTrigger value="tab-selection-rules-alignment">Alignment</TabsTrigger>
                  </TabsList>
                  <TabsContent value="tab-selection-rules-class">
                    <SelectionRulesSectionComponent characterId={id} type="Class" />
                    <SelectionRulesSectionComponent characterId={id} type="SubClass" />
                    {/* <SelectionRulesSectionComponent characterId={id} type="Proficiency" /> */}
                  </TabsContent>
                  <TabsContent value="tab-selection-rules-race">
                    <SelectionRulesSectionComponent characterId={id} type="Species" />
                    <SelectionRulesSectionComponent characterId={id} type="Background" />
                  </TabsContent>
                  {/* <TabsContent value="tab-selection-rules-background"></TabsContent> */}
                  <TabsContent value="tab-selection-rules-proficiency">
                    <SelectionRulesSectionComponent characterId={id} type="Proficiency" />
                  </TabsContent>
                  <TabsContent value="tab-selection-rules-language">
                    <SelectionRulesSectionComponent characterId={id} type="Language" />
                  </TabsContent>
                  <TabsContent value="tab-selection-rules-alignment">
                    <SelectionRulesSectionComponent characterId={id} type="Alignment" />
                  </TabsContent>
                </Tabs>
              </div>
            </CardContent>
          </Card>
        </CardWrapper>
      </div>
    </>
  );
}
