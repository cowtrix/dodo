import React from "react";
import PropTypes from "prop-types";

import { DateTile } from "../../date-tile";
import { Title } from "./title";
import { Description } from "./description";
import styles from "./summary.module.scss";
import { Button } from "../../button";

export const Summary = ({
	Name,
	location = "Glasgow",
	StartDate,
	EndDate,
	PublicDescription,
	GUID
}) => (
	<li className={styles.eventSummmary}>
		<Button to={"/rebellion/" + GUID} className={styles.link}>
			<DateTile
				startDate={new Date(StartDate)}
				endDate={new Date(EndDate)}
			/>
			<Title title={Name} location={location} />
			<Description description={PublicDescription} />
		</Button>
	</li>
);

Summary.propTypes = {
	Name: PropTypes.string,
	location: PropTypes.string,
	StartDate: PropTypes.string,
	EndDate: PropTypes.string,
	summary: PropTypes.string
};
