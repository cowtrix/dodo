import React, { Fragment, useState } from "react";
import { Button } from "app/components";
import { Dialog } from "./dialog";
import { Selector } from "./selector";

import styles from "./events.module.scss";

export const Events = ({ eventTypes, eventsFiltered, searchFilterEvents }) => {
	const [dialogOpen, setDialogOpen] = useState(false);

	return (
		<Fragment>
			<div className={styles.mobile}>
				<Button variant="primary" onClick={() => setDialogOpen(true)}>
					Events
				</Button>
				<Dialog
					active={dialogOpen}
					closeDialog={() => setDialogOpen(false)}
					eventTypes={eventTypes}
					eventsFiltered={eventsFiltered}
					searchFilterEvents={searchFilterEvents}
				/>
			</div>
			<div className={styles.desktop}>
				<Selector
					eventTypes={eventTypes}
					eventsFiltered={eventsFiltered}
					searchFilterEvents={searchFilterEvents}
				/>
			</div>
		</Fragment>
	);
};
