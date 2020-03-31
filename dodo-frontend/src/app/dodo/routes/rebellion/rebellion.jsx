import React, { useEffect, useState } from "react";
import PropTypes from "prop-types";
import {
	fetchRebellion,
	fetchRebellionEvents
} from "app/domain/services/rebellion";

import { RebellionDetail, RebellionEvents } from "app/components";
import styles from "./rebellion.module.scss";

export const Rebellion = ({ match }) => {
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
		<div className={styles.wrapper}>
			<div className={styles.rebellion}>
				<div className={styles.detail}>
					<RebellionDetail rebellion={rebellion} />
				</div>
				<div className={styles.events}>
					<RebellionEvents events={events} />
				</div>
			</div>
		</div>
	);
};

Rebellion.propTypes = {
	match: PropTypes.object.isRequired
};
