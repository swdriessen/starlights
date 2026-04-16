"use client"

import {
  CampaignBrief,
  CampaignBriefTopImage,
  CampaignQuestLog,
  CampaignQuestLogTopImage,
  CampaignRoster,
  CampaignRosterTopImage,
} from "./cards/campaign-cards"

export default function WipExample() {
  return (
    <>
      <div className="flex border-spacing-4 flex-row flex-wrap gap-10">
        <div className="flex w-88 flex-col gap-10">
          <CampaignRoster />
          <CampaignRosterTopImage />
        </div>
        <div className="flex w-88 flex-col gap-10">
          <CampaignQuestLog />
          <CampaignQuestLogTopImage />
        </div>
        <div className="flex w-88 flex-col gap-10">
          <CampaignBrief />
          <CampaignBriefTopImage />
        </div>
      </div>
    </>
  )
}
