export function LoadingPanel({ message: _message }: { message: string }) {
  return (
    <div className="loading-overlay">
      <svg className="notebook-loader" viewBox="0 0 100 115" width="110" height="110" fill="none" xmlns="http://www.w3.org/2000/svg">
        <rect x="15" y="14" width="78" height="96" rx="5" fill="rgba(0,0,0,0.07)" />
        <rect x="12" y="11" width="78" height="96" rx="5" fill="var(--card-bg)" stroke="var(--accent)" strokeWidth="2" />
        <rect x="12" y="11" width="13" height="96" rx="5" fill="var(--accent)" opacity="0.15" />
        <line x1="25" y1="11" x2="25" y2="107" stroke="var(--accent)" strokeWidth="1.5" opacity="0.4" />
        {[25, 38, 51, 64, 77, 90].map((y) => (
          <circle key={y} cx="18.5" cy={y} r="3.2" fill="var(--card-bg)" stroke="var(--accent)" strokeWidth="1.5" />
        ))}
        <rect x="30" y="19" width="52" height="7" rx="2" fill="var(--accent)" opacity="0.18" />
        <line x1="30" y1="40" x2="84" y2="40" stroke="var(--accent)" strokeWidth="2" strokeLinecap="round" strokeDasharray="54" className="nb-l1" />
        <line x1="30" y1="54" x2="84" y2="54" stroke="var(--accent)" strokeWidth="2" strokeLinecap="round" strokeDasharray="54" className="nb-l2" />
        <line x1="30" y1="68" x2="84" y2="68" stroke="var(--accent)" strokeWidth="2" strokeLinecap="round" strokeDasharray="54" className="nb-l3" />
        <line x1="30" y1="82" x2="70" y2="82" stroke="var(--accent)" strokeWidth="2" strokeLinecap="round" strokeDasharray="40" className="nb-l4" />
        <g className="nb-pen">
          <rect x="-4" y="-19" width="8" height="17" rx="3.5" fill="var(--accent-strong)" />
          <rect x="-4" y="-4" width="8" height="3" rx="1" fill="#d4a017" />
          <polygon points="-3.5,-1 3.5,-1 0,5" fill="var(--accent)" />
          <rect x="-2" y="-17" width="1.5" height="10" rx="1" fill="white" opacity="0.45" />
        </g>
      </svg>
    </div>
  );
}
