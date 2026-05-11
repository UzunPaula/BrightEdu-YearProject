export function LoadingPanel({ message }: { message: string }) {
  return (
    <div className="panel">
      <p className="muted">{message}</p>
    </div>
  );
}
