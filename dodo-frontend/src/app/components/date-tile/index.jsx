import React from "react";

import styles from "./date-tile.module.scss";

const DateTile = ({ startDate, endDate = null, backgroundColor }) => {
	const splitStartDate = `${startDate}`.split(" ");

	if (!endDate) {
		return (
			<div className={styles.DateTile} style={{ backgroundColor }}>
				<div>{splitStartDate[2]}</div>
				<strong>{splitStartDate[1]}</strong>
			</div>
		);
	}
	const splitEndDate = `${endDate}`.split(" ");

	if (endDate.getMonth() === startDate.getMonth()) {
		return (
			<div className={styles.DateTile} style={{ backgroundColor }}>
				<div>
					{splitStartDate[2]} - {splitEndDate[2]}
				</div>
				<strong>{splitStartDate[1]}</strong>
			</div>
		);
	}
	return (
		<div className={styles.DateTile} style={{ backgroundColor }}>
			<strong>
				{splitStartDate[2]} - {splitStartDate[1]}
			</strong>
			<strong>
				- {splitEndDate[2]} - {splitEndDate[1]}
			</strong>
		</div>
	);
};

export default DateTile;
