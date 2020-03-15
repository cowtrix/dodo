import React from "react";

import DateTile from "app/components/date-tile";
import PageTitle from "app/components/page-title";
import styles from "./rebellion-detail.module.scss";

const RebellionDetail = ({ rebellion }) => {
	return (
		<div className={styles.RebellionDetail}>
			<div className={styles.heading}>
				<DateTile
					startDate={new Date(rebellion.StartDate)}
					endDate={new Date(rebellion.EndDate)}
					backgroundColor="#8dd7cf"
        />
        <div className={styles.title}>
          <PageTitle title={rebellion.Name} />
        </div>
			</div>
		</div>
	);
};

export default RebellionDetail;
