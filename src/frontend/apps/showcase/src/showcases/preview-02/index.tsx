import { AccountAccess } from "./cards/account-access"
import { CardOverview } from "./cards/card-overview"
import { ClaimableBalance } from "./cards/claimable-balance"
import { ContributionHistory } from "./cards/contribution-history"
import { CoverArt } from "./cards/cover-art"
import { DividendIncome } from "./cards/dividend-income"
import { EmptyConnectBank } from "./cards/empty-connect-bank"
import { EmptyDistributeTrack } from "./cards/empty-distribute-track"
import { EmptyExploreCatalog } from "./cards/empty-explore-catalog"
import { Faq } from "./cards/faq"
import { FrontDoor } from "./cards/front-door"
import { IndexInvesting } from "./cards/index-investing"
import { KitchenIsland } from "./cards/kitchen-island"
import { LoadingCard } from "./cards/loading-card"
import { NewMilestone } from "./cards/new-milestone"
import { NotificationSettings } from "./cards/notification-settings"
import { Payments } from "./cards/payments"
import { PayoutThreshold } from "./cards/payout-threshold"
import { PowerUsage } from "./cards/power-usage"
import { Preferences } from "./cards/preferences"
// import { QrConnect } from "./cards/qr-connect"
import { ReceivingMethod } from "./cards/receiving-method"
import { RecentTransactions } from "./cards/recent-transactions"
import { ReleaseCatalog } from "./cards/release-catalog"
import { RollerShades } from "./cards/roller-shades"
import { SavingsProgress } from "./cards/savings-progress"
import { SavingsTargets } from "./cards/savings-targets"
import { SidebarNav } from "./cards/sidebar-nav"
import { SocialLinks } from "./cards/social-links"
import { StockPerformance } from "./cards/stock-performance"
import { SyncingState } from "./cards/syncing-state"
import { TransferFunds } from "./cards/transfer-funds"
import { UpcomingPayments } from "./cards/upcoming-payments"

export default function Preview02Example() {
  return (
    <div className="3xl:[--gap:--spacing(12)] style-lyra:md:[--gap:--spacing(6)] style-mira:md:[--gap:--spacing(6)] overflow-x-auto overflow-y-hidden bg-muted contain-[paint] [--gap:--spacing(4)] md:[--gap:--spacing(10)] dark:bg-background">
      <div className="flex w-full min-w-max justify-center">
        <div
          className="style-lyra:md:w-[2600px] style-mira:md:w-[2600px] grid w-[2400px] grid-cols-7 items-start gap-(--gap) bg-muted p-(--gap) md:w-[3000px] dark:bg-background *:[div]:gap-(--gap)"
          data-slot="capture-target"
        >
          <div className="flex flex-col p-1 [contain-intrinsic-size:380px_1200px] [content-visibility:auto]">
            <ContributionHistory />
            <EmptyDistributeTrack />
            {/* <QrConnect /> */}
            <DividendIncome />
            <IndexInvesting />
            <SyncingState />
          </div>
          <div className="flex flex-col p-1 [contain-intrinsic-size:380px_1200px] [content-visibility:auto]">
            <PayoutThreshold />
            <ClaimableBalance />
            <Preferences />
            <SavingsProgress />
            <KitchenIsland />
          </div>
          <div className="col-span-2 flex flex-col p-1 [contain-intrinsic-size:760px_1200px] [content-visibility:auto]">
            <SavingsTargets />
            <RecentTransactions />
            <div className="grid grid-cols-2 items-start gap-(--gap)">
              <div className="flex flex-col gap-(--gap)">
                <SidebarNav />
                <Faq />
              </div>
              <div className="flex flex-col gap-(--gap)">
                <Payments />
                <FrontDoor />
              </div>
            </div>
            <ReleaseCatalog />
          </div>
          <div className="flex flex-col p-1 [contain-intrinsic-size:380px_1200px] [content-visibility:auto]">
            <AccountAccess />
            <CardOverview />
            <TransferFunds />
            <CoverArt />
            <LoadingCard />
          </div>
          <div className="flex flex-col p-1 [contain-intrinsic-size:380px_1200px] [content-visibility:auto]">
            <ReceivingMethod />
            <PowerUsage />
            <EmptyConnectBank />
            <UpcomingPayments />
            <RollerShades />
          </div>
          <div className="flex flex-col p-1 [contain-intrinsic-size:380px_1200px] [content-visibility:auto]">
            <StockPerformance />
            <EmptyExploreCatalog />
            <NewMilestone />
            <SocialLinks />
            <NotificationSettings />
          </div>
        </div>
      </div>
    </div>
  )
}
