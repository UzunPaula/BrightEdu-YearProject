import { useState } from "react";
import { useTranslation } from "react-i18next";

type DialogState = { message: string; resolve: (value: boolean) => void } | null;

export function useConfirm() {
  const { t } = useTranslation();
  const [dialog, setDialog] = useState<DialogState>(null);

  const confirm = (message: string): Promise<boolean> =>
    new Promise(resolve => setDialog({ message, resolve }));

  const handleResult = (value: boolean) => {
    dialog?.resolve(value);
    setDialog(null);
  };

  const dialogNode = dialog ? (
    <div className="dialog-backdrop">
      <div className="dialog" role="dialog" aria-modal="true">
        <p className="dialog-message">{dialog.message}</p>
        <div className="dialog-actions">
          <button className="btn-danger" onClick={() => handleResult(true)}>{t("common.confirm")}</button>
          <button className="btn-secondary" onClick={() => handleResult(false)}>{t("common.cancel")}</button>
        </div>
      </div>
    </div>
  ) : null;

  return { confirm, dialogNode };
}
