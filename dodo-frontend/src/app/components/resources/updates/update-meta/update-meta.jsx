import styles from "./update-meta.module.scss";
import { dateFormatted, timeFormatted } from "../../header/dates/services";
import React from "react";

export const UpdateMeta = ({ timestamp, source }) => (
	<div className={styles.updateMeta}>
		{timeFormatted(timestamp, { timeStyle: "short" })} -{" "}
		{dateFormatted(timestamp, {
			day: "2-digit",
			month: "long",
			year: "numeric",
		})}
		{source ? ` via ${source}` : null}
	</div>
);
