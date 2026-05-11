import { useTranslation } from "react-i18next";

export function ErrorPanel({ message }: { message: string }) {
  const { t } = useTranslation();
  return (
    <div className="panel error-panel">
      <strong>{t("common.errorOccurred")}</strong>
      <p className="muted">{message}</p>
    </div>
  );
}
