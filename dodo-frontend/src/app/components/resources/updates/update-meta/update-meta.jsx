import styles from "../updates.module.scss";
import { dateFormatted, timeFormatted } from "../../header/dates/services";
import React from "react";

export const UpdateMeta = ({ timestamp }) => (
	<div className={styles.updateMeta}>
		{timeFormatted(timestamp, { timeStyle: "short" })} -{" "}
		{dateFormatted(timestamp, {
			day: "2-digit",
			month: "long",
			year: "numeric",
		})}
	</div>
);
