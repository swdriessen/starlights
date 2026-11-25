/** Decorative rings for the armillary star map; drawn behind the links. */
export function Armillary() {
  const cx = 500
  const cy = 280
  return (
    <g stroke="currentColor" fill="none" strokeOpacity="0.28">
      <g className="spin-slow">
        <ellipse cx={cx} cy={cy} rx={330} ry={330} />
        <ellipse cx={cx} cy={cy} rx={330} ry={110} strokeOpacity="0.5" />
        <ellipse cx={cx} cy={cy} rx={110} ry={330} strokeOpacity="0.5" />
      </g>
      <ellipse cx={cx} cy={cy} rx={250} ry={250} strokeDasharray="2 8" />
      <ellipse cx={cx} cy={cy} rx={170} ry={170} strokeDasharray="2 8" />
      <ellipse cx={cx} cy={cy} rx={380} ry={380} strokeOpacity="0.15" />
      <line x1={cx - 380} y1={cy} x2={cx + 380} y2={cy} strokeOpacity="0.14" />
      <line x1={cx} y1={cy - 280} x2={cx} y2={cy + 280} strokeOpacity="0.14" />
    </g>
  )
}
