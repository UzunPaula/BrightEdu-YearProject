import { useState } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faEye, faEyeSlash } from "@fortawesome/free-solid-svg-icons";

type Props = {
  value: string;
  onChange: (e: React.ChangeEvent<HTMLInputElement>) => void;
  required?: boolean;
  inputStyle: React.CSSProperties;
  onFocus?: (e: React.FocusEvent<HTMLInputElement>) => void;
  onBlur?: (e: React.FocusEvent<HTMLInputElement>) => void;
};

export function PasswordInput({ value, onChange, required, inputStyle, onFocus, onBlur }: Props) {
  const [visible, setVisible] = useState(false);

  return (
    <div style={{ position: "relative" }}>
      <input
        type={visible ? "text" : "password"}
        value={value}
        onChange={onChange}
        required={required}
        style={{ ...inputStyle, paddingRight: "2.8rem" }}
        onFocus={onFocus}
        onBlur={onBlur}
      />
      <button
        type="button"
        onClick={() => setVisible(v => !v)}
        tabIndex={-1}
        style={{
          position: "absolute", right: "0.85rem", top: "50%",
          transform: "translateY(-50%)",
          background: "none", border: "none", cursor: "pointer",
          color: "var(--muted)", padding: 0, lineHeight: 1,
          display: "grid", placeItems: "center",
        }}
        aria-label={visible ? "Ascunde parola" : "Arată parola"}
      >
        <FontAwesomeIcon icon={visible ? faEyeSlash : faEye} style={{ fontSize: "0.95rem" }} />
      </button>
    </div>
  );
}
