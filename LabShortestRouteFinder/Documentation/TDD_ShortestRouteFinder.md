**Author** : @name

**Date** : YYYY-MM-DD

**Epic** : link to project management (JIRA/Linear) ticket

***A note** — not all sections are necessary for every project. Include them anyway to document the full thought process. Not all questions need to be answered for every project; skip the irrelevant ones. We're looking for thoroughness of communication and detailed thought process, not unnecessary information.*

## Introduction

### Background

Summary of the problem you are solving. What are you trying to accomplish? How will you solve it? What’s wrong with things the way they are now? Who are the stakeholders?

### Current Functionality

If the project is an update to the app and not a new feature, how does it work now?

### In Scope

What is being addressed in this epic? What are you expected solve?

### Out of Scope

If there are related problems that you have decided not to address with this epic, but which someone might conceivably expect you to solve, then list them here.

### Assumptions & Dependencies

What are you assuming will be true or in place to make your solution successful?

### Terminology

If the document uses any special words or terms, list them here.

## Considerations

### Concerns

What are your high-level concerns with this project? What issues do you think would come up in the execution of this project? What impact would this project have on the end user (Customers, Shops, Internal Departments, etc)?

### Operational Readiness Considerations

How will your chosen solution be deployed? What metrics and alarms will be key to monitoring the health of your solution? Will there be any throttling or blacklisting mechanisms in place? Will there be any data recovery mechanisms in place? If this is a multi-tenant solution, how are you dealing with noisy neighbor issues? How will your solution be debugged when problems occur? How will your solution recover in case of a brown-out? Are there any operational tools required for your solution? What are the blockers that would keep the project from moving forward? Blockers that you might encounter along the way?

### Open Questions

If there are any risks or unknowns, list them here. Also if there is additional research to be done, mention that as well. Are there any considerations which could impact your design for which you do not currently have answers? How are you going to get answers? Will any required team members be loaned to other teams during the time slated for implementation? Are all of the required dependencies available in all the regions you need them? What are the one way doors, and are we sure we want to go through them?

### Cross-Region Considerations

If applicable, how does your solution optimize or is your solution compatible with cross-region requirements. This includes data transfer costs between regions, availability of the service in different data centers, latency issues, etc.

## Proposed Design

### Solution

A brief, high-level description of the solution. The following sections will go into more detail.

### System Architecture

If the design consists of a collaboration between multiple large-scale components, list those components here — or better, include a diagram.

### Data Model

Describe how the data is stored. This could include a description of the database schema or a json.

If separate people are handling the FE / BE, communication is key. Arrange for an early-in-project meeting to agree on request/response formats  (with maybe an additional ticket to hold the data returned to the FE & the endpoint, to surface that information/possible conversation sooner).

### Interface / API Definitions

Describe how the various components talk to each other. For example, if there are REST endpoints, describe the endpoint URL and the format of the data and parameters used.

### Business Logic

If the design requires any non-trivial algorithms or logic, describe them.

### Migration Strategy

If the design incurs non-backwards-compatible changes to an existing system, describe the process whereby entities that depend on the system are going to migrate to the new design.

### Work Required

Include a high level breakdown of the work required to implement your proposed solution, including t-shirt size estimates (S, M, XL) where appropriate. Also, specifically call out if this solution requires resources from other teams to be completed (away teams, dependencies etc.)

### Work Sequence

Include a high-level work sequence— what portions of this project need to be code-complete and merged into a release branch before

### High-level Test Plan

At a high level, describe how your chosen solution should be tested.

Work with the QA team to set up a demo, PR party, and QA schedule.

### Deployment Sequence

If a user story in the epic is dependent on another user story's deployment, please list that sequence here. Also, if a migration needs to occur for this project, work with Ops to schedule that in advance of the user story deployment sequence.

Create a new Launch Checklist for your project on the day of the launch.

## Impact

Describe the potential impacts of the design on overall performance, security, and other aspects of the system.

### Cost Analysis

A high level analysis of the costs that will be incurred in running your chosen solution on a day-to-day basis.

### Cross-Region Considerations

If applicable, how does your solution optimize or is compatible with cross-region requirements? This includes data transfer costs between regions, availability of the service in different data centers, latency issues, etc.

## Alternatives

What alternatives have you considered and discarded? Why don’t these work?

## Looking into the Future

What do you see as the next steps of this project? What are the next iterations? Any Nice to Haves that need to be discussed?