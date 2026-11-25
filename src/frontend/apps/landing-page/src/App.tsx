import { BuilderSection } from "./components/builder-section"
import { Hero } from "./components/hero"
import { FollowSection, SiteFooter } from "./components/site-footer"
import { SiteHeader } from "./components/site-header"
import { SkySection } from "./components/sky-section"

export function App() {
  return (
    <div className="landing">
      <SiteHeader />
      <main>
        <Hero />
        <BuilderSection />
        <SkySection />
        <FollowSection />
      </main>
      <SiteFooter />
    </div>
  )
}

export default App
