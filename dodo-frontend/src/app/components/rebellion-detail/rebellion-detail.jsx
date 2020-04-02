import React from "react";

import { DateTile } from "app/components/date-tile";
import { PageTitle } from "app/components/page-title";
import { Button } from "app/components/button";
import styles from "./rebellion-detail.module.scss";

export const RebellionDetail = ({ rebellion }) => {
	return (
		<div className={styles.RebellionDetail}>
			<div className={styles.heading}>
				<DateTile
					startDate={new Date(rebellion.StartDate)}
					endDate={new Date(rebellion.EndDate)}
				/>
				<div className={styles.title}>
					<PageTitle title={rebellion.Name} subTitle="Glasgow" />
				</div>
			</div>
			<div className={styles.detail}>{rebellion.PublicDescription}</div>
			<div className={styles.join}>
				<Button
					to={"/rebellion/join"}
					type="button"
					style={{ display: "inline-block" }}
				>
					JOIN REBELLION
				</Button>
			</div>
		</div>
	);
};
