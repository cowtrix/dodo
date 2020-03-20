import React, { useEffect, useState } from "react";
import {
	fetchRebellion,
	fetchRebellionEvents
} from "app/domain/services/rebellion";
import PropTypes from "prop-types";

import RebellionDetail from "app/components/rebellion-detail";
import RebellionEvents from "app/components/rebellion-events";

import styles from "./rebellion.module.scss";

const Rebellion = ({ match }) => {
	const { rebellionId } = match.params;
	const [rebellion, setRebellion] = useState();
	const [events, setEvents] = useState();

	useEffect(() => {
		const load = async () => {
			setRebellion(await fetchRebellion(rebellionId));
			setEvents(await fetchRebellionEvents(rebellionId));
		};
		load();
	}, [rebellionId]);

	if (!rebellion) {
		return <div>Loading</div>;
	}

	return (
		<div className={styles.rebellion}>
			<div className={styles.detail}>
				<RebellionDetail rebellion={rebellion} />
			</div>
			<div className={styles.events}>
				<RebellionEvents events={events} />
			</div>
		</div>
	);
};

Rebellion.propTypes = {
	match: PropTypes.object.isRequired
};

export default Rebellion;
